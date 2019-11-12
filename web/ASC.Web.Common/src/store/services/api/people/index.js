
import { request } from "../client";
//import axios from "axios";
import Filter from "./filter";

export function getUserList(filter = Filter.getDefault()) {
    const params =
      filter && filter instanceof Filter
        ? `/filter.json?${filter.toUrlParams()}`
        : "";
  
    return request({
      method: "get",
      url: `/people${params}`
    });
  }