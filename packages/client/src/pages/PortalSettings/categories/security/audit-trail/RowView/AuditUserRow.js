import React from "react";
import Row from "@docspace/components/row";
import { AuditContent } from "./AuditContent";

export const AuditUserRow = ({ item, sectionWidth }) => {
  return (
    <Row sectionWidth={sectionWidth} key={item.id} data={item}>
      <AuditContent item={item} sectionWidth={sectionWidth} />
    </Row>
  );
};
