import EmptyScreenPersonSvgUrl from "PUBLIC_DIR/images/empty_screen_persons.svg?url";
import EmptyScreenPersonSvgDarkUrl from "PUBLIC_DIR/images/empty_screen_persons_dark.svg?url";
import React from "react";
import { inject, observer } from "mobx-react";

import Text from "@docspace/components/text";
import { StyledNoItemContainer } from "../../styles/noItem";

const NoAccountsItem = ({ t, theme }) => {
  const imgSrc = theme.isBase
    ? EmptyScreenPersonSvgUrl
    : EmptyScreenPersonSvgDarkUrl;

  return (
    <StyledNoItemContainer>
      <div className="no-thumbnail-img-wrapper">
        <img src={imgSrc} />
      </div>
      <Text className="no-item-text" textAlign="center">
        {t("InfoPanel:AccountsEmptyScreenText")}
      </Text>
    </StyledNoItemContainer>
  );
};

export default inject(({ auth }) => {
  return {
    theme: auth.settingsStore.theme,
  };
})(observer(NoAccountsItem));
