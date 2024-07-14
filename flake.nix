{
  inputs = {
    nixpkgs.url = "github:NixOS/nixpkgs/nixpkgs-unstable";
    systems.url = "github:nix-systems/default";
    devenv.url = "github:cachix/devenv";
  };

  nixConfig = {
    extra-trusted-public-keys = "devenv.cachix.org-1:w1cLUi8dv3hnoSPGAuibQv+f9TZLr6cv/Hm9XgU50cw=";
    extra-substituters = "https://devenv.cachix.org";
  };

  outputs = { self, nixpkgs, devenv, systems, ... } @ inputs:
    let
      forEachSystem = nixpkgs.lib.genAttrs (import systems);
      dotnet = pkgs: pkgs.dotnet-sdk_8;
    in
    {
      packages = forEachSystem (system:
        let
          pkgs = nixpkgs.legacyPackages.${system};
        in
        {
          default = pkgs.writeShellScriptBin "dotnet-run" ''
            cd SampleSolrApp
            ${dotnet pkgs}/bin/dotnet run -v minimal
          '';

          build = pkgs.writeShellScriptBin "dotnet-run" ''
            ${dotnet pkgs}/bin/dotnet build
          '';
        }
      );

      devShells = forEachSystem
        (system:
          let
            pkgs = nixpkgs.legacyPackages.${system};
          in
          {
            default = devenv.lib.mkShell {
              inherit inputs pkgs;
              modules = [
                {
                  # https://devenv.sh/reference/options/
                  packages = [
                    (dotnet pkgs)
                    pkgs.bashInteractive
                  ] ++ [
                    (pkgs.vscode-with-extensions.override {
                        vscodeExtensions = with pkgs.vscode-extensions; [
                          ms-dotnettools.csharp
                          jnoortheen.nix-ide
                        ];
                    })
                  ];

                  enterShell = ''
                    code .
                  '';
                }
              ];
            };
          });
    };
}
