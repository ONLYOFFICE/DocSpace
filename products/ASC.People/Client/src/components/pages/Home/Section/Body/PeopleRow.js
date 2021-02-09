import React from "react";
import { Row, Avatar } from "asc-web-components";
import UserContent from "./userContent";

const PeopleRow = ({
  man,
  widthProp,
  selectGroup,
  isAdmin,
  currentUserId,
  context,
  isMobile,
  history,
  settings,
  getUserContextOptions,
  onContentRowSelect,
  needForUpdate,
}) => {
  //console.log("PeopleRow render");
  const { checked, role, displayName, avatar, id, status, options } = man;
  const sectionWidth = context.sectionWidth;
  const showContextMenu = options && options.length > 0;
  const contextOptionsProps =
    (isAdmin && showContextMenu) || (showContextMenu && id === currentUserId)
      ? {
          contextOptions: getUserContextOptions(options, id),
        }
      : {};

  const checkedProps = checked !== null && isAdmin ? { checked } : {};

  const element = (
    <Avatar size="min" role={role} userName={displayName} source={avatar} />
  );
  return (
    <Row
      key={id}
      status={status}
      data={man}
      element={element}
      onSelect={onContentRowSelect}
      {...checkedProps}
      {...contextOptionsProps}
      needForUpdate={needForUpdate}
      sectionWidth={sectionWidth}
    >
      <UserContent
        isMobile={isMobile}
        widthProp={widthProp}
        user={man}
        history={history}
        settings={settings}
        selectGroup={selectGroup}
        sectionWidth={sectionWidth}
      />
    </Row>
  );
};
export default PeopleRow;
