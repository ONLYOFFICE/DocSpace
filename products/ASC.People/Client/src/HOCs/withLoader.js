import React, { useEffect, useState } from "react";
import { observer, inject } from "mobx-react";
import { isMobile } from "react-device-detect";

import Loaders from "@appserver/common/components/Loaders";

let loadTimeout = null;
export default function withLoader(WrappedComponent) {
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

    return firstLoad || !isLoaded || (isMobile && inLoad) || !tReady ? (
      <Loaders.Rows isRectangle={false} />
    ) : (
      <WrappedComponent {...props} />
    );
  };

  return inject(({ auth, peopleStore }) => {
    const { isLoading, firstLoad } = peopleStore;
    return {
      isLoaded: auth.isLoaded,
      isLoading,
      firstLoad,
    };
  })(observer(withLoader));
}
