import React, { useEffect, useState } from "react";
import { observer, inject } from "mobx-react";
import { useNavigate } from "react-router-dom";
import Loader from "@docspace/components/loader";
import axios from "axios";
import { combineUrl } from "@docspace/common/utils";
import ConfirmWrapper from "./ConfirmWrapper";

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

      getAuthProviders,
      getCapabilities,
    } = props;
    const [inLoad, setInLoad] = useState(false);

    const type = linkData ? linkData.type : null;
    const confirmHeader = linkData ? linkData.confirmHeader : null;

    const navigate = useNavigate();

    useEffect(() => {
      if (
        (type === "PasswordChange" ||
          type === "LinkInvite" ||
          type === "Activation" ||
          type === "EmpInvite") &&
        !passwordSettings
      ) {
        axios
          .all([getSettings(), getPortalPasswordSettings(confirmHeader)])
          .catch((error) => {
            let errorMessage = "";
            if (typeof error === "object") {
              errorMessage =
                error?.response?.data?.error?.message ||
                error?.statusText ||
                error?.message ||
                "";
            } else {
              errorMessage = error;
            }

            console.error(errorMessage);
            navigate(
              combineUrl(
                window.DocSpaceConfig?.proxy?.url,
                `/login/error?message=${errorMessage}`
              )
            );
          });
      }
    }, [passwordSettings]);

    useEffect(() => {
      if (type === "LinkInvite" || type === "EmpInvite") {
        axios.all([getAuthProviders(), getCapabilities()]).catch((error) => {
          let errorMessage = "";
          if (typeof error === "object") {
            errorMessage =
              error?.response?.data?.error?.message ||
              error?.statusText ||
              error?.message ||
              "";
          } else {
            errorMessage = error;
          }
          console.error(errorMessage);
          navigate(
            combineUrl(
              window.DocSpaceConfig?.proxy?.url,
              `/login/error?message=${errorMessage}`
            )
          );
        });
      }
    }, []);

    const isLoaded =
      type === "TfaActivation" || type === "TfaAuth"
        ? props.isLoaded
        : type === "PasswordChange" ||
          type === "LinkInvite" ||
          type === "Activation" ||
          type === "EmpInvite"
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
      <ConfirmWrapper>
        <WrappedComponent {...props} />
      </ConfirmWrapper>
    );
  };

  return inject(({ auth, confirm }) => {
    const { isLoaded, isLoading } = confirm;
    const { passwordSettings, getSettings, getPortalPasswordSettings } =
      auth.settingsStore;
    const { getAuthProviders, getCapabilities } = auth;

    return {
      isLoaded,
      isLoading,
      getSettings,
      passwordSettings,
      getPortalPasswordSettings,
      getAuthProviders,
      getCapabilities,
    };
  })(observer(withLoader));
}
