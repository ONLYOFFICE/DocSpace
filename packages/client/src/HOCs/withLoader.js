import React, { useEffect, useState } from "react";
import { observer, inject } from "mobx-react";
import { isMobile } from "react-device-detect";

import Loaders from "@docspace/common/components/Loaders";

const pathname = window.location.pathname.toLowerCase();
const isEditor = pathname.indexOf("doceditor") !== -1;
const isGallery = pathname.indexOf("form-gallery") !== -1;

let loadTimeout = null;
const withLoader = (WrappedComponent) => (Loader) => {
  const withLoader = (props) => {
    const {
      tReady,
      firstLoad,
      isLoaded,
      isLoading,
      viewAs,
      setIsBurgerLoading,
      isLoadingFilesFind,
    } = props;
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

    useEffect(() => {
      if ((!isEditor && firstLoad) || !isLoaded || (isMobile && inLoad)) {
        setIsBurgerLoading(true);
      } else {
        setIsBurgerLoading(false);
      }
    }, [isEditor, firstLoad, isLoaded, isMobile, inLoad]);

    return (!isEditor && firstLoad && !isGallery) ||
      !isLoaded ||
      (isMobile && inLoad) ||
      (isLoadingFilesFind && !Loader) ||
      !tReady ? (
      Loader ? (
        Loader
      ) : viewAs === "tile" ? (
        <Loaders.Tiles />
      ) : viewAs === "table" ? (
        <Loaders.TableLoader />
      ) : (
        <Loaders.Rows />
      )
    ) : (
      <WrappedComponent {...props} />
    );
  };

  return inject(({ auth, filesStore }) => {
    const { firstLoad, isLoading, viewAs, isLoadingFilesFind } = filesStore;
    const { settingsStore } = auth;
    const { setIsBurgerLoading } = settingsStore;
    return {
      firstLoad,
      isLoaded: auth.isLoaded,
      isLoading,
      viewAs,
      setIsBurgerLoading,
      isLoadingFilesFind,
    };
  })(observer(withLoader));
};
export default withLoader;
