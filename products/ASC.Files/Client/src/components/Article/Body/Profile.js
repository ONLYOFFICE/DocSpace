import React from "react";
import styled from "styled-components";
import { inject, observer } from "mobx-react";
import Avatar from "@appserver/components/avatar";
import Text from "@appserver/components/text";
import ContextMenuButton from "@appserver/components/context-menu-button";

const StyledProfile = styled.div`
  position: fixed;
  bottom: 0;
  padding: 16px 0;
  display: flex;
  align-items: center;
  flex-flow: row wrap;
  gap: 16px;

  .option-button {
    margin-left: auto;
  }
`;

const Profile = (props) => {
  const { user } = props;
  console.log(user);

  const getUserRole = (user) => {
    let isModuleAdmin = user.listAdminModules && user.listAdminModules.length;

    if (user.isOwner) return "owner";
    if (user.isAdmin || isModuleAdmin) return "admin";
    if (user.isVisitor) return "guest";
    return "user";
  };

  const userRole = getUserRole(user);

  const getContextOptions = () => {
    return [];
  };

  return (
    <StyledProfile>
      <Avatar
        size="small"
        role={userRole}
        source={user.avatar}
        userName={user.displayName}
      />
      <Text fontSize="12px" fontWeight={600}>
        {user.displayName}
      </Text>
      <div className="option-button">
        <ContextMenuButton
          zIndex={402}
          directionX="left"
          directionY="top"
          iconName="images/vertical-dots.react.svg"
          size={15}
          isFill
          getData={getContextOptions}
          isDisabled={false}
          usePortal={true}
        />
      </div>
    </StyledProfile>
  );
};

export default inject(({ auth }) => {
  const { userStore } = auth;
  const { user } = userStore;

  return { user };
})(observer(Profile));
