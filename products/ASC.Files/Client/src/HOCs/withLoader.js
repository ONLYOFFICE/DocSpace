import React, { useEffect, useState } from "react";
import { observer, inject } from "mobx-react";
import { isMobile } from "react-device-detect";

import Loaders from "@appserver/common/components/Loaders";

let loadTimeout = null;
const withLoader = (WrappedComponent) => (Loader) => {
  const withLoader = (props) => {
    const { tReady, firstLoad, isLoaded, isLoading, viewAs } = props;
    const [inLoad, setInLoad] = useState(false);

    const cleanTimer = () => {
      loadTimeout && clearTimeout(loadTimeout);
      loadTimeout = null;
    };

    useEffect(() => {
      if (isLoading) {
        cleanTimer();
        loadTimeout = setTimeout(() => {
          //console.log("inLoad", true);
          setInLoad(true);
        }, 500);
      } else {
        cleanTimer();
        //console.log("inLoad", false);
        setInLoad(false);
      }

      return () => {
        cleanTimer();
      };
    }, [isLoading]);

    return firstLoad || !isLoaded || (isMobile && inLoad) || !tReady ? (
      Loader ? (
        Loader
      ) : viewAs === "tile" ? (
        <Loaders.Tiles />
      ) : (
        <Loaders.Rows />
      )
    ) : (
      <WrappedComponent {...props} />
    );
  };

  return inject(({ auth, filesStore }) => {
    const { firstLoad, isLoading, viewAs } = filesStore;
    return {
      firstLoad,
      isLoaded: auth.isLoaded,
      isLoading,
      viewAs,
    };
  })(observer(withLoader));
};
export default withLoader;
