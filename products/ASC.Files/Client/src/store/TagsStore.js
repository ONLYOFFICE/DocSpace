import { makeAutoObservable } from "mobx";
import api from "@appserver/common/api";
import { AppServerConfig, RoomsType } from "@appserver/common/constants";

class TagsStore {
  tags = [];

  constructor() {
    makeAutoObservable(this);
  }

  setTags = (tags) => {
    this.tags = tags;
  };

  createTag = (name) => {
    return api.rooms.createTag(name);
  };

  fetchTags = () => {
    const request = () =>
      api.rooms.getTags().then((res) => {
        this.setTags(res);
        return res;
      });

    return request();
  };
}

export default TagsStore;
