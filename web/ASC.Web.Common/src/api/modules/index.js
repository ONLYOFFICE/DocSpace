import { request } from "../client";
import axios from "axios";
import config from "../../../package.json";

export function getModulesList() {
  return request({
    method: "get",
    url: "/modules",
  }).then((modules) => {
    return (
      modules &&
      axios
        .all(
          modules.map((m) =>
            request({
              method: "get",
              url: `${window.location.origin}/${m.replace(
                "api/2.0",
                config.api.url
              )}`,
            }).catch((err) => {
              return Promise.resolve(err);
            })
          )
        )
        .then((modules) => {
          const workingModules = modules.filter(
            (module) => typeof module === "object"
          );
          const newModules = workingModules.map((m) => {
            return {
              ...m,
              isPrimary: true,
              iconUrl: m.link + "images/icon.svg",
              imageUrl: m.link + m.imageUrl,
            };
          });

          return newModules;
        })
    );
  });
}
