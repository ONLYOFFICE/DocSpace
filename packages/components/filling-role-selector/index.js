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
  const { roles } = props;

  const mockRoles = [
    { id: 3, role: "Director", order: 3, color: "#BB85E7" },
    { id: 2, role: "Accountant", order: 2, color: "#70D3B0" },
    { id: 1, role: "Employee", order: 1, color: "#FBCC86", everyone: true },
  ];

  const cloneRoles = JSON.parse(JSON.stringify(mockRoles));
  const sortedInOrderRoles = cloneRoles.sort((a, b) =>
    a.order > b.order ? 1 : -1
  );

  const onAddRole = () => {
    console.log("click onAddRole");
  };

  const itemEveryoneRole = mockRoles.find((item) => item.everyone == true);

  //TODO: Add correct translations
  return (
    <StyledFillingRoleSelector {...props}>
      {itemEveryoneRole && (
        <>
          <StyledRow>
            <StyledNumber>{itemEveryoneRole.order}</StyledNumber>
            <StyledEveryoneRoleIcon />
            <StyledEveryoneRoleContainer>
              <div className="title">
                <StyledRole>{itemEveryoneRole.role}</StyledRole>
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
            <StyledAddRoleButton onClick={onAddRole} color={item.color} />
            <StyledRole>{item.role}</StyledRole>
          </StyledRow>
        );
      })}
    </StyledFillingRoleSelector>
  );
};

FillingRoleSelector.propTypes = {};

FillingRoleSelector.defaultProps = {};

export default FillingRoleSelector;
