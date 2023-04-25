import React, { useEffect, useState } from "react";
import { observer, inject } from "mobx-react";

let loadTimeout = null;
const delayLoader = 300;

const withLoader = (WrappedComponent) => (Loader) => {
  const withLoader = (props) => {
    const {
      tReady,
      isLoaded,
      isLoading,
      firstLoad,
      profileLoaded,
      setIsBurgerLoading,
      isLoadedProfileSectionBody,
      setIsLoadedProfileSectionBody,
    } = props;
    const [inLoad, setInLoad] = useState(true);
    const isProfileViewLoader = Loader.props.isProfileView;
    const isProfileFooterLoader = Loader.props.isProfileFooter;

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
        }, delayLoader);
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
      if (!isLoaded) {
        setIsBurgerLoading(true);
      } else {
        setIsBurgerLoading(false);
      }
    }, [isLoaded]);

    useEffect(() => {
      if (!isProfileViewLoader) return;

      if (!(firstLoad || !isLoaded || inLoad || !tReady || !profileLoaded)) {
        setIsLoadedProfileSectionBody(true);
      } else {
        setIsLoadedProfileSectionBody(false);
      }
    }, [
      firstLoad,
      isLoaded,
      inLoad,
      tReady,
      profileLoaded,
      setIsLoadedProfileSectionBody,
      isProfileViewLoader,
    ]);

    return <WrappedComponent {...props} />;
  };

  return inject(({ auth, peopleStore }) => {
    const { isLoaded, settingsStore } = auth;
    const {
      loadingStore,
      isLoadedProfileSectionBody,
      setIsLoadedProfileSectionBody,
    } = peopleStore;
    const { isLoading, firstLoad, profileLoaded } = loadingStore;
    const { setIsBurgerLoading } = settingsStore;
    return {
      isLoaded,
      isLoading,
      firstLoad,
      profileLoaded,
      setIsBurgerLoading,
      isLoadedProfileSectionBody,
      setIsLoadedProfileSectionBody,
    };
  })(observer(withLoader));
};

export default withLoader;
