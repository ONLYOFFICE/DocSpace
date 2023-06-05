import React, { useEffect, useState } from "react";
import { observer, inject } from "mobx-react";
import { isMobile } from "react-device-detect";
import Loaders from "@docspace/common/components/Loaders";

const pathname = window.location.pathname.toLowerCase();
const isEditor = pathname.indexOf("doceditor") !== -1;
const isGallery = pathname.indexOf("form-gallery") !== -1;

const withLoader = (WrappedComponent) => (Loader) => {
  const withLoader = (props) => {
    const {
      isInit,
      tReady,
      firstLoad,
      isLoaded,

      viewAs,
      showBodyLoader,
      isLoadingFilesFind,
    } = props;

    return (!isEditor && firstLoad && !isGallery) ||
      !isLoaded ||
      showBodyLoader ||
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

  return inject(({ auth, filesStore, clientLoadingStore }) => {
    const { viewAs, isLoadingFilesFind, isInit } = filesStore;
    const { firstLoad, isLoading, showBodyLoader } = clientLoadingStore;
    const { settingsStore } = auth;
    const { setIsBurgerLoading } = settingsStore;
    return {
      firstLoad,
      isLoaded: auth.isLoaded,
      isLoading,
      viewAs,
      setIsBurgerLoading,
      isLoadingFilesFind,
      isInit,
      showBodyLoader,
    };
  })(observer(withLoader));
};
export default withLoader;
