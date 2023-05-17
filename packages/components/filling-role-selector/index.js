import React from "react";
import PropTypes from "prop-types";
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

import { ReactSVG } from "react-svg";
const FillingRoleSelector = (props) => {
  const { roles, users, onClick, onRemoveUser } = props;

  const cloneRoles = JSON.parse(JSON.stringify(roles));
  const sortedInOrderRoles = cloneRoles.sort((a, b) =>
    a.order > b.order ? 1 : -1
  );
  const everyoneRole = roles.find((item) => item.everyone == true);

  //TODO: Add correct translations
  //TODO: Rename item
  //TODO: Rename  onClick
  return (
    <StyledFillingRoleSelector {...props}>
      {everyoneRole && (
        <>
          <StyledRow>
            <StyledNumber>{everyoneRole.order}</StyledNumber>
            <StyledEveryoneRoleIcon />
            <StyledEveryoneRoleContainer>
              <div className="title">
                <StyledRole>{everyoneRole.role}</StyledRole>
                <StyledAssignedRole>@Everyone</StyledAssignedRole>
              </div>

              <div className="role-description">
                The form is available for filling out by all participants of
                this room.
              </div>
            </StyledEveryoneRoleContainer>
          </StyledRow>
          <StyledTooltip>
            Each enrolled by users from the first videos, the form should be
            offered to users, the one offered below.
          </StyledTooltip>
        </>
      )}

      {sortedInOrderRoles.map((item, index) => {
        if (item.everyone) return;

        if (users) {
          const roleWithUser = users.find((user) => user.role === item.role);
          if (roleWithUser) {
            return (
              <StyledUserRow key={index}>
                <div className="content">
                  <StyledNumber>{item.order}</StyledNumber>
                  <StyledAvatar
                    src={
                      roleWithUser.hasAvatar
                        ? roleWithUser.avatar
                        : AvatarBaseReactSvgUrl
                    }
                  />
                  <div className="user-with-role">
                    <StyledRole>
                      {roleWithUser.firstName} {roleWithUser.lastName}
                    </StyledRole>
                    <StyledAssignedRole>{roleWithUser.role}</StyledAssignedRole>
                  </div>
                </div>

                <ReactSVG
                  src={RemoveSvgUrl}
                  onClick={() => onRemoveUser(roleWithUser.id)}
                />
              </StyledUserRow>
            );
          } else {
            return (
              <StyledRow key={index}>
                <StyledNumber>{item.order}</StyledNumber>
                <StyledAddRoleButton onClick={onClick} color={item.color} />
                <StyledRole>{item.role}</StyledRole>
              </StyledRow>
            );
          }
        }

        return (
          <StyledRow key={index}>
            <StyledNumber>{item.order}</StyledNumber>
            <StyledAddRoleButton onClick={onClick} color={item.color} />
            <StyledRole>{item.role}</StyledRole>
          </StyledRow>
        );
      })}
    </StyledFillingRoleSelector>
  );
};

FillingRoleSelector.propTypes = {
  roles: PropTypes.array,
  users: PropTypes.array,
  onClick: PropTypes.func,
  onRemoveUser: PropTypes.func,
};

FillingRoleSelector.defaultProps = {};

export default FillingRoleSelector;
