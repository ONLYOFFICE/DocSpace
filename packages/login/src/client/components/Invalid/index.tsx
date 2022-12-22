import React from "react";
import { inject, observer } from "mobx-react";
import { useHistory, Link } from "react-router-dom";
import ErrorContainer from "@docspace/common/components/ErrorContainer";
import Text from "@docspace/components/text";
import { useTranslation, Trans } from "react-i18next";
import { combineUrl } from "@docspace/common/utils";
import { Dark, Base } from "@docspace/components/themes";

const homepage = "/login";

type InvalidErrorProps = {
  theme?: Record<string, string>
  setTheme?: (theme: object) => void
}


const InvalidError = ({ theme, setTheme }: InvalidErrorProps) => {
  const [hydrated, setHydrated] = React.useState(false);

  const [proxyHomepageUrl, setProxyHomepageUrl] = React.useState("");
  const { t } = useTranslation("Login");
  const history = useHistory();

  React.useLayoutEffect(() => {
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
      history.push(url)
    }, 10000);
    return () => clearTimeout(timeout)
  }, []);

  React.useEffect(() => {
    setHydrated(true);
  }, [])


  return (
    <>
      {hydrated &&
        (<ErrorContainer headerText={t("ErrorInvalidHeader")} theme={theme}>
          <Text theme={theme} fontSize="13px" fontWeight="600">
            <Trans t={t} i18nKey="ErrorInvalidText">
              In 10 seconds you will be redirected to the
              <Link
                className="error_description_link"
                to={proxyHomepageUrl}
              >
                DocSpace
              </Link>
            </Trans>
          </Text>
        </ErrorContainer>)
      }
    </>
  );
};

export default inject(({ loginStore }: any) => {
  return {
    theme: loginStore.theme,
    setTheme: loginStore.setTheme,
  };
})(observer(InvalidError));





