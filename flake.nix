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
      packagesf = pkgs: with pkgs; [
        dotnet-sdk_6
        bashInteractive
      ];
    in
    {
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
                  packages = packagesf pkgs ++ [
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
