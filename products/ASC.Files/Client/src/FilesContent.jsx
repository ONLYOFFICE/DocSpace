import React, { useEffect } from "react";
import { useStore } from "react-redux";
import dynamic from "@redux-dynostore/react-redux";
import { attachReducer } from "@redux-dynostore/core";
//import rootReducer from "./store/rootReducer";
//import portalReducer from "./store/portal/reducers";
const FilesContent = (props) => {
  const store = useStore();

  // useEffect(() => {
  //   console.log("Store object", store);
  //   store.reducerManager.add("portal", portalReducer);

  //   return store.reducerManager.remove("portal");
  // }, []);

  return <div>FILES PAGE</div>;
};

export default FilesContent;

//export default dynamic("portal", attachReducer(portalReducer))(FilesContent);
