import React, { useEffect } from "react";
import { Box, Text } from "@appserver/components/src";
// import { useStore } from "react-redux";
// import dynamic from "@redux-dynostore/react-redux";
// import { attachReducer } from "@redux-dynostore/core";
//import rootReducer from "./store/rootReducer";
//import portalReducer from "./store/portal/reducers";
const FilesContent = (props) => {
  //const store = useStore();

  // useEffect(() => {
  //   console.log("Store object", store);
  //   store.reducerManager.add("portal", portalReducer);

  //   return store.reducerManager.remove("portal");
  // }, []);

  return (
    <Box
      displayProp="flex"
      flexDirection="column"
      alignItems="center"
      widthProp="100%"
    >
      <Box displayProp="flex" alignItems="center" heightProp="100%">
        <Text fontSize="24px" color="blue">
          FILES PAGE
        </Text>
      </Box>
    </Box>
  );
};

export default FilesContent;

//export default dynamic("portal", attachReducer(portalReducer))(FilesContent);
