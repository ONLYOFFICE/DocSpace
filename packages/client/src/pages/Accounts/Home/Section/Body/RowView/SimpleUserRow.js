import React from "react";
import { withRouter } from "react-router";

import Row from "@docspace/components/row";

import withContextOptions from "SRC_DIR/HOCs/withPeopleContextOptions";
import withContent from "SRC_DIR/HOCs/withPeopleContent";

import UserContent from "./userContent";

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
      mode={"modern"}
    >
      <UserContent {...props} />
    </Row>
  );
};

export default withRouter(withContextOptions(withContent(SimpleUserRow)));
