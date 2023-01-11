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
      isInit,
      tReady,
      firstLoad,
      isLoaded,
      isLoading,
      viewAs,
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

    return (!isEditor && firstLoad && !isGallery) ||
      !isLoaded ||
      (isMobile && inLoad) ||
      (isLoadingFilesFind && !Loader) ||
      !tReady ||
      !isInit ? (
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
    const {
      firstLoad,
      isLoading,
      viewAs,
      isLoadingFilesFind,
      isInit,
    } = filesStore;

    return {
      firstLoad,
      isLoaded: auth.isLoaded,
      isLoading,
      viewAs,
      isLoadingFilesFind,
      isInit,
    };
  })(observer(withLoader));
};
export default withLoader;
