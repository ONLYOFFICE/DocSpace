import { request } from "../client";
import { combineUrl } from "../../utils";

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
        iconUrl: combineUrl(m.link, m.iconUrl),
        imageUrl: combineUrl(m.link, m.imageUrl),
      };
    });

    return newModules;
  });
}
