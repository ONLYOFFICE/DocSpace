import { request } from "../client";

export function getModulesList() {
  return request({
    method: "get",
    url: "/modules/info",
  }).then((modules) => {
    const workingModules = modules.filter(
      (module) => typeof module === "object"
    );
    const newModules = workingModules.map((m) => {
      return {
        ...m,
        iconUrl: m.link + "images/icon.svg",
        imageUrl: m.link + m.imageUrl,
      };
    });

    return newModules;
  });
}
