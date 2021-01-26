import React, { useEffect } from "react";
import { useStore } from "react-redux";
import peopleReducer from "./store/people/reducers";
import profileReducer from "./store/profile/reducers";
import groupReducer from "./store/group/reducers";
import portalReducer from "./store/portal/reducers";

const PeopleContent = (props) => {
  const store = useStore();

  useEffect(() => {
    store.attachReducer("people", peopleReducer);
    store.attachReducer("profile", profileReducer);
    store.attachReducer("group", groupReducer);
    store.attachReducer("portal", portalReducer);
    return () => {
      store.detachReducer("people");
      store.detachReducer("profile");
      store.detachReducer("group");
      store.detachReducer("portal");
    };
  }, []);

  return <div>PEOPLE PAGE</div>;
};

export default PeopleContent;
