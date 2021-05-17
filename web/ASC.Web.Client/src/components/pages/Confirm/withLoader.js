import React, { useEffect, useState } from "react";
import { observer, inject } from "mobx-react";
import Loader from "@appserver/components/loader";
//import { isMobile } from "react-device-detect";

let loadTimeout = null;
export default function withLoader(WrappedComponent, type) {
  const withLoader = (props) => {
    const { tReady, isLoaded, isLoading } = props;
    const [inLoad, setInLoad] = useState(false);

    const cleanTimer = () => {
      loadTimeout && clearTimeout(loadTimeout);
      loadTimeout = null;
    };

    useEffect(() => {
      if (isLoading) {
        cleanTimer();
        loadTimeout = setTimeout(() => {
          setInLoad(true);
        }, 500);
      } else {
        cleanTimer();
        setInLoad(false);
      }

      return () => {
        cleanTimer();
      };
    }, [isLoading]);

    return !isLoaded || !tReady || inLoad ? (
      <Loader className="pageLoader" type="rombs" size="40px" />
    ) : (
      <WrappedComponent {...props} />
    );
  };

  return inject(({ confirm }) => {
    return {
      isLoaded: confirm.isLoaded,
      isLoading: confirm.isLoading,
    };
  })(observer(withLoader));
}
