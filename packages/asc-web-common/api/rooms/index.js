import { request } from "../client";

export function getRooms(filter) {
  const options = {
    method: "get",
    url: `/files/rooms?${filter.toApiUrlParams()}`,
  };

  return request(options).then((res) => {
    return res;
  });
}
