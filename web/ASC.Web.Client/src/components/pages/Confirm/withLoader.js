import React, { useEffect, useState } from "react";
import { observer, inject } from "mobx-react";
import Loader from "@appserver/components/loader";
import axios from "axios";
import { combineUrl } from "@appserver/common/utils";
import { AppServerConfig } from "@appserver/common/constants";

let loadTimeout = null;
export default function withLoader(WrappedComponent) {
  const withLoader = (props) => {
    const {
      tReady,
      isLoading,
      linkData,
      passwordSettings,
      getSettings,
      getPortalPasswordSettings,
      history,
    } = props;
    const [inLoad, setInLoad] = useState(false);

    const type = linkData ? linkData.type : null;
    const confirmHeader = linkData ? linkData.confirmHeader : null;

    useEffect(() => {
      if (
        (type === "PasswordChange" || type === "LinkInvite") &&
        !passwordSettings
      ) {
        axios
          .all([getSettings(), getPortalPasswordSettings(confirmHeader)])
          .catch((error) => {
            console.error(error);
            history.push(
              combineUrl(AppServerConfig.proxyURL, `/login/error=${error}`)
            );
          });
      }
    }, [passwordSettings]);

    const isLoaded =
      type === "TfaActivation" || type === "TfaAuth"
        ? props.isLoaded
        : type === "PasswordChange" || type === "LinkInvite"
        ? !!passwordSettings
        : true;

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

    return !isLoaded || !tReady ? (
      <Loader className="pageLoader" type="rombs" size="40px" />
    ) : (
      <WrappedComponent {...props} />
    );
  };

  return inject(({ auth, confirm }) => {
    const { isLoaded, isLoading } = confirm;
    const {
      passwordSettings,
      getSettings,
      getPortalPasswordSettings,
    } = auth.settingsStore;

    return {
      isLoaded,
      isLoading,
      getSettings,
      passwordSettings,
      getPortalPasswordSettings,
    };
  })(observer(withLoader));
}
