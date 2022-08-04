import path from "path";
import fs from "fs";

export const getAssets = (): object => {
  const manifest = fs.readFileSync(
    path.join(__dirname, "client/manifest.json"),
    "utf-8"
  );

  const assets = JSON.parse(manifest);

  return assets;
};

export const getScripts = (assets: object): string[] => {
  const regTest = /static\/js\/.*/;
  const keys = [];
  console.log(assets);
  for (let key in assets) {
    if (assets.hasOwnProperty(key) && regTest.test(key)) {
      keys.push(key);
    }
  }

  return keys;
};
