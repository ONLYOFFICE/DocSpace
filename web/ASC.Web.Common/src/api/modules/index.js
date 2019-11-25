import { request } from "../client";
import axios from "axios";

export function getModulesList() {
  return request({
    method: "get",
    url: "/modules"
  }).then(modules => {
    return (
      modules &&
      axios.all(
        modules.map(m =>
          request({
            method: "get",
            url: `${window.location.origin}/${m}`
          })
        )
      )
    );
  });
}
