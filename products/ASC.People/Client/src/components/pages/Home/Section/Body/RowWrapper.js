import React, { memo } from "react";
import { areEqual } from "react-window";
import PeopleRow from "./PeopleRow";

const RowWrapper = memo(({ data, index, style }) => {
  console.log("Data", data);
  const man = data.peopleList[index];
  console.log("RowWrapper render");
  return (
    <div style={style}>
      <PeopleRow
        man={man}
        widthProp={data.widthProp}
        selectGroup={data.selectGroup}
        isAdmin={data.isAdmin}
        currentUserId={data.currentUserId}
        context={data.context}
        isMobile={data.isMobile}
        history={data.history}
        settings={data.settings}
        getUserContextOptions={data.getUserContextOptions}
        onContentRowSelect={data.onContentRowSelect}
        needForUpdate={data.needForUpdate}
      />
    </div>
  );
}, areEqual);

export default RowWrapper;
