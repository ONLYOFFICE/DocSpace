import React from "react";
import Row from "@docspace/components/row";
import UserContent from "./userContent";
import { withRouter } from "react-router";
import withContextOptions from "../../../../../HOCs/withPeopleContextOptions";
import withContent from "../../../../../HOCs/withPeopleContent";

const SimpleUserRow = (props) => {
  const {
    item,
    sectionWidth,
    contextOptionsProps,
    checkedProps,
    onContentRowSelect,
    element,
  } = props;

  return (
    <Row
      key={item.id}
      data={item}
      element={element}
      onSelect={onContentRowSelect}
      {...checkedProps}
      {...contextOptionsProps}
      sectionWidth={sectionWidth}
    >
      <UserContent {...props} />
    </Row>
  );
};

export default withRouter(withContextOptions(withContent(SimpleUserRow)));
