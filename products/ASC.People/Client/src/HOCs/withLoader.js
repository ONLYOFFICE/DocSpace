import React, { useEffect, useState } from "react";
import { observer, inject } from "mobx-react";
import { isMobile } from "react-device-detect";

import Loaders from "@appserver/common/components/Loaders";

let loadTimeout = null;
export default function withLoader(WrappedComponent, Loader) {
  const withLoader = (props) => {
    const {
      tReady,
      isLoaded,
      isLoading,
      firstLoad,
      match,
      profile,
      loaded,
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

    return <WrappedComponent {...props} />;

    /*switch (match.path) {
      case "/products/people/filter":
        return firstLoad || !isLoaded || (isMobile && inLoad) || !tReady ? (
          <Loaders.Rows isRectangle={false} />
        ) : (
          <WrappedComponent {...props} />
        );
      case "/products/people/view/:userId":
        return firstLoad || !profile || !tReady || inLoad ? (
          <Loaders.ProfileView />
        ) : (
          <WrappedComponent {...props} />
        );
      case "/products/people/create/:type":
        return firstLoad || !tReady || !loaded || !isLoaded || inLoad ? (
          <Loaders.ProfileView isEdit={false} />
        ) : (
          <WrappedComponent {...props} />
        );

      case "/products/people/edit/:userId":
        return firstLoad || !tReady || (loaded && inLoad) ? (
          <Loaders.ProfileView isEdit={false} />
        ) : (
          <WrappedComponent {...props} />
        );
      case "/products/people/group/create":
        return firstLoad || !isLoaded || !tReady || inLoad ? (
          <Loaders.Group />
        ) : (
          <WrappedComponent {...props} />
        );
      default:
        return <WrappedComponent {...props} />;
    }*/
  };

  return inject(({ auth, peopleStore }) => {
    const { isLoading, firstLoad } = peopleStore;
    return {
      isLoaded: auth.isLoaded,
      isLoading,
      firstLoad,
      profile: peopleStore.targetUserStore.targetUser,
    };
  })(observer(withLoader));
}
