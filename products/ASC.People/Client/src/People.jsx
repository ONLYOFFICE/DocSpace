import React, { useEffect, useState } from "react";
import { useStore } from "react-redux";
import peopleReducer from "./store/people/reducers";
import profileReducer from "./store/profile/reducers";
import groupReducer from "./store/group/reducers";
import portalReducer from "./store/portal/reducers";
import Routes from "./Routes";

const PeopleContent = (props) => {
  const [isLoading, setIsLoading] = useState(true);
  const store = useStore();
  console.log("People props", props, store);

  useEffect(() => {
    store.attachReducer("people", peopleReducer);
    store.attachReducer("profile", profileReducer);
    store.attachReducer("group", groupReducer);
    store.attachReducer("portal", portalReducer);
    setIsLoading(false);
    return () => {
      store.detachReducer("people");
      store.detachReducer("profile");
      store.detachReducer("group");
      store.detachReducer("portal");
    };
  }, []);

  return isLoading ? <div>LOADING STORE</div> : <Routes />;
};

export default PeopleContent;
