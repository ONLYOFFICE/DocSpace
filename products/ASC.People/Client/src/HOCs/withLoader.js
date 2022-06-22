import React, { useEffect, useState } from "react";
import { observer, inject } from "mobx-react";

let loadTimeout = null;
const withLoader = (WrappedComponent) => (Loader) => {
  const withLoader = (props) => {
    const {
      tReady,
      isLoaded,
      isLoading,
      firstLoad,
      profileLoaded,
      setIsBurgerLoading,
    } = props;
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

    useEffect(() => {
      if (firstLoad || !isLoaded || inLoad || !profileLoaded) {
        setIsBurgerLoading(true);
      } else {
        setIsBurgerLoading(false);
      }
    }, [firstLoad, isLoaded, inLoad, profileLoaded]);

    return firstLoad || !isLoaded || inLoad || !tReady || !profileLoaded ? (
      Loader
    ) : (
      <WrappedComponent {...props} />
    );
  };

  return inject(({ auth, peopleStore }) => {
    const { isLoaded, settingsStore } = auth;
    const { loadingStore } = peopleStore;
    const { isLoading, firstLoad, profileLoaded } = loadingStore;
    const { setIsBurgerLoading } = settingsStore;
    return {
      isLoaded,
      isLoading,
      firstLoad,
      profileLoaded,
      setIsBurgerLoading,
    };
  })(observer(withLoader));
};

export default withLoader;
