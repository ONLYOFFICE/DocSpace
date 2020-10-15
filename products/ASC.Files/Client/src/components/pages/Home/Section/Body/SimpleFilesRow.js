import React from "react";
import styled from "styled-components";
import { Row } from "asc-web-components";
import { connect } from "react-redux";
import { withRouter } from "react-router";
import { setSelected, setSelection } from "../../../../../store/files/actions";
import { getSelected } from "../../../../../store/files/selectors";

const StyledSimpleFilesRow = styled(Row)`
  ${(props) =>
    !props.contextOptions &&
    `
    & > div:last-child {
        width: 0px;
      }
  `}

  .share-button-icon {
    margin-right: 8px;
  }

  .share-button,
  .share-button-icon:hover {
    cursor: pointer;
    div {
      color: "#657077";
    }
  }
`;

const SimpleFilesRow = (props) => {
  const { children, selected, setSelected, setSelection, data, ...rest } = props;

  const onSelectItem = () => {
    selected === "close" && setSelected("none");
    setSelection([data]);
  };

  return (
    <StyledSimpleFilesRow {...rest} data={data} selectItem={onSelectItem}>
      {children}
    </StyledSimpleFilesRow>
  );
};

const mapStateToProps = (state) => {
  return {
    selected: getSelected(state),
  };
};

export default connect(mapStateToProps, {
  setSelection,
  setSelected,
})(withRouter(SimpleFilesRow));
