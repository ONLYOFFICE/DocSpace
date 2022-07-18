import React from "react";
import { inject, observer } from "mobx-react";
import { withRouter } from "react-router";
import { useTranslation } from "react-i18next";
import Avatar from "@appserver/components/avatar";
import Text from "@appserver/components/text";
import ContextMenuButton from "@appserver/components/context-menu-button";
import { isTablet } from "@appserver/components/utils/device";
import { StyledArticleProfile } from "../styled-article";

const ArticleProfile = (props) => {
  const { user, showText, getUserRole, getActions } = props;
  const { t } = useTranslation("Common");

  const tablet = isTablet();
  const avatarSize = tablet ? "min" : "base";
  const userRole = getUserRole(user);

  return (
    <StyledArticleProfile showText={showText} tablet={tablet}>
      <Avatar
        size={avatarSize}
        role={userRole}
        source={user.avatar}
        userName={user.displayName}
      />
      {(!tablet || showText) && (
        <>
          <Text fontSize="12px" fontWeight={600} noSelect>
            {user.displayName}
          </Text>
          <div className="option-button">
            <ContextMenuButton
              zIndex={402}
              directionX="left"
              directionY="top"
              iconName="/static/images/vertical-dots.react.svg"
              size={15}
              isFill
              getData={() => getActions(t)}
              isDisabled={false}
              usePortal={true}
            />
          </div>
        </>
      )}
    </StyledArticleProfile>
  );
};

export default withRouter(
  inject(({ auth, profileActionsStore }) => {
    const { getActions, getUserRole } = profileActionsStore;

    return {
      user: auth.userStore.user,
      getUserRole,
      getActions,
    };
  })(observer(ArticleProfile))
);
