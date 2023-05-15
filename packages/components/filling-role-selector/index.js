import React from "react";
import PropTypes from "prop-types";
import {
  StyledFillingRoleSelector,
  StyledRow,
  StyledNumber,
  StyledAddRoleButton,
  StyledRole,
} from "./styled-filling-role-selector";

const FillingRoleSelector = (props) => {
  const { roles } = props;

  const mockRoles = [
    { id: 1, role: "Employee" },
    { id: 2, role: "Accountant" },
    { id: 3, role: "Director" },
  ];

  const mockColorAddButton = [
    { id: 1, color: "#FBCC86" },
    { id: 2, color: "#70D3B0" },
    { id: 3, color: "#BB85E7" },
  ];

  const onAddRole = () => {
    console.log("click onAddRole");
  };

  return (
    <StyledFillingRoleSelector {...props}>
      {mockRoles.map((item, index) => {
        return (
          <StyledRow>
            <StyledNumber>{index + 1}</StyledNumber>
            <StyledAddRoleButton
              onClick={onAddRole}
              color={mockColorAddButton[index].color}
            />
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
