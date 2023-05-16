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
} from "./styled-filling-role-selector";

const FillingRoleSelector = (props) => {
  const { roles, users, onClick } = props;

  const cloneRoles = JSON.parse(JSON.stringify(roles));
  const sortedInOrderRoles = cloneRoles.sort((a, b) =>
    a.order > b.order ? 1 : -1
  );
  const everyoneRole = roles.find((item) => item.everyone == true);

  //TODO: Add correct translations
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
                <StyledRole className="comment">(@Everyone)</StyledRole>
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

      {sortedInOrderRoles.map((item) => {
        if (item.everyone) return;
        return (
          <StyledRow>
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
};

FillingRoleSelector.defaultProps = {};

export default FillingRoleSelector;
