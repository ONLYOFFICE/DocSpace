import React, { useEffect, useState } from "react";
import { observer, inject } from "mobx-react";

let loadTimeout = null;
const withLoader = (WrappedComponent) => (Loader) => {
  const withLoader = (props) => {
    const { tReady, isLoaded, isLoading, firstLoad, profileLoaded } = props;
    const [inLoad, setInLoad] = useState(true);

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

    return firstLoad || !isLoaded || inLoad || !tReady || !profileLoaded ? (
      Loader
    ) : (
      <WrappedComponent {...props} />
    );
  };

  return inject(({ auth, peopleStore }) => {
    const { isLoaded } = auth;
    const { loadingStore } = peopleStore;
    const { isLoading, firstLoad, profileLoaded } = loadingStore;
    return {
      isLoaded,
      isLoading,
      firstLoad,
      profileLoaded,
    };
  })(observer(withLoader));
};

export default withLoader;
