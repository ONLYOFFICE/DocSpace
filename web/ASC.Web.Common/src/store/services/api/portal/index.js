import { request } from "../client";

export function getShortenedLink(link) {
    return request({
      method: "put",
      url: "/portal/getshortenlink.json",
      data: link
    });
  }