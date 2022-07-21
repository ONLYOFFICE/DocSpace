import React, { useState, useRef } from "react";
import { inject, observer } from "mobx-react";
import { withRouter } from "react-router";
import { useTranslation } from "react-i18next";
import Avatar from "@appserver/components/avatar";
import Text from "@appserver/components/text";
import ContextMenuButton from "@appserver/components/context-menu-button";
import ContextMenu from "@appserver/components/context-menu";
import { isTablet } from "@appserver/components/utils/device";
import { StyledArticleProfile } from "../styled-article";

const ArticleProfile = (props) => {
  const { user, showText, getUserRole, getActions } = props;
  const { t } = useTranslation("Common");
  const [isOpen, setIsOpen] = useState(false);
  const ref = useRef(null);
  const menuRef = useRef(null);

  const tablet = isTablet();
  const avatarSize = tablet ? "min" : "base";
  const userRole = getUserRole(user);

  const toggle = (e, isOpen) => {
    isOpen ? menuRef.current.show(e) : menuRef.current.hide(e);
    setIsOpen(isOpen);
  };

  const onAvatarClick = (e) => {
    if (tablet && !showText) toggle(e, !isOpen);
  };

  const onHide = () => {
    setIsOpen(false);
  };

  const model = getActions(t);

  return (
    <StyledArticleProfile showText={showText} tablet={tablet}>
      <div ref={ref}>
        <Avatar
          size={avatarSize}
          role={userRole}
          source={user.avatar}
          userName={user.displayName}
          onClick={onAvatarClick}
        />
        <ContextMenu
          model={model}
          containerRef={ref}
          ref={menuRef}
          onHide={onHide}
          scaled={false}
          leftOffset={-50}
        />
      </div>
      {(!tablet || showText) && (
        <>
          <Text className="userName" fontWeight={600} noSelect truncate>
            {user.displayName}
          </Text>
          <ContextMenuButton
            className="option-button"
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
