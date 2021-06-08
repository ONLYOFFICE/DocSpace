import React, { useEffect, useState } from "react";
import { observer, inject } from "mobx-react";

let loadTimeout = null;
const WithLoader = (WrappedComponent) => (Loader) => {
  const withLoader = (props) => {
    const { tReady, isLoaded, isLoading, firstLoad } = props;
    const [inLoad, setInLoad] = useState(false);

    const cleanTimer = () => {
      loadTimeout && clearTimeout(loadTimeout);
      loadTimeout = null;
    };

    useEffect(() => {
      if (isLoading) {
        cleanTimer();
        loadTimeout = setTimeout(() => {
          console.log("inLoad", true);
          setInLoad(true);
        }, 500);
      } else {
        cleanTimer();
        console.log("inLoad", false);
        setInLoad(false);
      }

      return () => {
        cleanTimer();
      };
    }, [isLoading]);

    return !firstLoad && isLoaded && !inLoad && tReady ? (
      <WrappedComponent {...props} />
    ) : (
      Loader
    );
  };

  return inject(({ auth, peopleStore }) => {
    const { isLoaded } = auth;
    const { loadingStore } = peopleStore;
    const { isLoading, firstLoad } = loadingStore;
    return {
      isLoaded,
      isLoading,
      firstLoad,
    };
  })(observer(withLoader));
};

export default WithLoader;
