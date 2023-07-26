import React from "react";
import { inject, observer } from "mobx-react";
import { useNavigate, Link } from "react-router-dom";
import ErrorContainer from "@docspace/common/components/ErrorContainer";
import Text from "@docspace/components/text";
import { useTranslation, Trans } from "react-i18next";
import { combineUrl } from "@docspace/common/utils";
import { Dark, Base } from "@docspace/components/themes";
import useIsomorphicLayoutEffect from "../../hooks/useIsomorphicLayoutEffect";
import { getMessageFromKey, getMessageKeyTranslate } from "../../helpers/utils";

const homepage = "/login";

interface InvalidErrorProps {
  theme?: Record<string, string>;
  setTheme?: (theme: object) => void;
  match?: {
    params: MatchType;
  };
}

const InvalidError = ({ theme, setTheme, match }: InvalidErrorProps) => {
  console.log(match);
  const [hydrated, setHydrated] = React.useState(false);

  const [proxyHomepageUrl, setProxyHomepageUrl] = React.useState("");
  const { t } = useTranslation(["Login", "Errors", "Common"]);
  const navigate = useNavigate();

  useIsomorphicLayoutEffect(() => {
    const themeCurrent =
      window.matchMedia &&
      window.matchMedia("(prefers-color-scheme: dark)").matches
        ? Dark
        : Base;
    setTheme?.(themeCurrent);
  }, []);

  React.useEffect(() => {
    const url = combineUrl(window.DocSpaceConfig?.proxy?.url, homepage);
    setProxyHomepageUrl(url);
    const timeout = setTimeout(() => {
      navigate(url);
    }, 10000);
    return () => clearTimeout(timeout);
  }, []);

  React.useEffect(() => {
    setHydrated(true);
  }, []);

  const message = getMessageFromKey(match?.messageKey);
  const errorTitle = match?.messageKey
    ? getMessageKeyTranslate(t, message)
    : t("ErrorInvalidHeader");

  return (
    <>
      {hydrated && (
        <ErrorContainer headerText={errorTitle} theme={theme}>
          <Text theme={theme} fontSize="13px" fontWeight="600">
            <Trans t={t} i18nKey="ErrorInvalidText">
              In 10 seconds you will be redirected to the
              <Link className="error_description_link" to={proxyHomepageUrl}>
                DocSpace
              </Link>
            </Trans>
          </Text>
        </ErrorContainer>
      )}
    </>
  );
};

export default inject(({ loginStore }: any) => {
  return {
    theme: loginStore.theme,
    setTheme: loginStore.setTheme,
  };
})(observer(InvalidError));
