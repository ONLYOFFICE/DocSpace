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
            .catch(err => {
              return Promise.resolve(err);
            })
        )
      )
        .then((modules) => {
          const workingModules = modules.filter(module => typeof module === 'object');
          const newModules = workingModules.map(m => {
            return {
              ...m,
              isPrimary: true,
              iconUrl: m.link + "images/icon.svg",
              imageUrl: m.link + m.imageUrl,
            };
          })
            .concat({
              id: "77777777-7777-7777-7777-777777777777",
              title: "Documents",
              link: "/coming-soon/",
              imageUrl: "/images/documents.svg",
              iconUrl: "/images/documentsIcon.svg",
              description: "Create, edit and share documents. Collaborate on them in real-time. 100% compatibility with MS Office formats guaranteed.",
              isPrimary: true,
              iconName: "DocumentsIcon"
            });

          return newModules;
        })
    );
  });
}
