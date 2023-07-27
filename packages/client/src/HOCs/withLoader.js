import React, { useEffect, useState } from "react";
import { observer, inject } from "mobx-react";
import { useLocation } from "react-router-dom";

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
      accountsViewAs,
    } = props;

    const location = useLocation();

    const currentViewAs = location.pathname.includes("/accounts/filter")
      ? accountsViewAs
      : viewAs;

    return (!isEditor && firstLoad && !isGallery) ||
      !isLoaded ||
      showBodyLoader ||
      (isLoadingFilesFind && !Loader) ||
      !tReady ||
      !isInit ? (
      Loader ? (
        Loader
      ) : currentViewAs === "tile" ? (
        <Loaders.Tiles />
      ) : currentViewAs === "table" ? (
        <Loaders.TableLoader />
      ) : (
        <Loaders.Rows />
      )
    ) : (
      <WrappedComponent {...props} />
    );
  };

  return inject(
    ({
      auth,
      filesStore,
      peopleStore,
      clientLoadingStore,
      publicRoomStore,
    }) => {
      const { viewAs, isLoadingFilesFind, isInit } = filesStore;
      const { viewAs: accountsViewAs } = peopleStore;

      const { firstLoad, isLoading, showBodyLoader } = clientLoadingStore;
      const { settingsStore } = auth;
      const { setIsBurgerLoading } = settingsStore;
      const { isPublicRoom } = publicRoomStore;

      return {
        firstLoad: isPublicRoom ? false : firstLoad,
        isLoaded: isPublicRoom ? true : auth.isLoaded,
        isLoading,
        viewAs,
        setIsBurgerLoading,
        isLoadingFilesFind,
        isInit: isPublicRoom ? true : isInit,
        showBodyLoader,
        isPublicRoom,
        accountsViewAs,
      };
    }
  )(observer(withLoader));
};
export default withLoader;
