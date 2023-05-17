import React from "react";
import PropTypes from "prop-types";
import { useTranslation } from "react-i18next";
import { ReactSVG } from "react-svg";
import {
  StyledFillingRoleSelector,
  StyledRow,
  StyledNumber,
  StyledAddRoleButton,
  StyledEveryoneRoleIcon,
  StyledRole,
  StyledEveryoneRoleContainer,
  StyledTooltip,
  StyledAssignedRole,
  StyledAvatar,
  StyledUserRow,
} from "./styled-filling-role-selector";
import AvatarBaseReactSvgUrl from "PUBLIC_DIR/images/avatar.base.react.svg?url";
import RemoveSvgUrl from "PUBLIC_DIR/images/remove.session.svg?url";

const FillingRoleSelector = (props) => {
  const { roles, users, onAddUser, onRemoveUser } = props;
  const { t } = useTranslation("Common");

  //If the roles in the roles array come out of order
  const cloneRoles = JSON.parse(JSON.stringify(roles));
  const sortedInOrderRoles = cloneRoles.sort((a, b) =>
    a.order > b.order ? 1 : -1
  );

  const everyoneRole = roles.find((item) => item.everyone);

  //TODO: Fix translations to correct ones when they appear on layouts
  const everyoneRoleNode = (
    <>
      <StyledRow>
        <StyledNumber>{everyoneRole.order}</StyledNumber>
        <StyledEveryoneRoleIcon />
        <StyledEveryoneRoleContainer>
          <div className="title">
            <StyledRole>{everyoneRole.name}</StyledRole>
            <StyledAssignedRole>{everyoneRole.everyone}</StyledAssignedRole>
          </div>
          <div className="role-description">
            {t("Common:DescriptionOfTheEveryoneRole")}
          </div>
        </StyledEveryoneRoleContainer>
      </StyledRow>
      <StyledTooltip>{t("Common:DescriptionOfTheRoleQueue")}</StyledTooltip>
    </>
  );

  return (
    <StyledFillingRoleSelector {...props}>
      {everyoneRole && everyoneRoleNode}
      {sortedInOrderRoles.map((role, index) => {
        if (role.everyone) return;
        const roleWithUser = users?.find((user) => user.role === role.name);

        return roleWithUser ? (
          <StyledUserRow key={index}>
            <div className="content">
              <StyledNumber>{role.order}</StyledNumber>
              <StyledAvatar
                src={
                  roleWithUser.hasAvatar
                    ? roleWithUser.avatar
                    : AvatarBaseReactSvgUrl
                }
              />
              <div className="user-with-role">
                <StyledRole>{roleWithUser.displayName}</StyledRole>
                <StyledAssignedRole>{roleWithUser.role}</StyledAssignedRole>
              </div>
            </div>
            <ReactSVG
              src={RemoveSvgUrl}
              onClick={() => onRemoveUser(roleWithUser.id)}
            />
          </StyledUserRow>
        ) : (
          <StyledRow key={index}>
            <StyledNumber>{role.order}</StyledNumber>
            <StyledAddRoleButton onClick={onAddUser} color={role.color} />
            <StyledRole>{role.name}</StyledRole>
          </StyledRow>
        );
      })}
    </StyledFillingRoleSelector>
  );
};

FillingRoleSelector.propTypes = {
  /** Array of roles */
  roles: PropTypes.array,
  /** Array of assigned users per role */
  users: PropTypes.array,
  /** The function of adding a user to a role */
  onAddUser: PropTypes.func,
  /** Function to remove a user from a role */
  onRemoveUser: PropTypes.func,
};

export default FillingRoleSelector;
