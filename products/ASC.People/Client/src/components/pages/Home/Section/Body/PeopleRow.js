import React from "react";
import { connect } from "react-redux";
import { Row, Avatar } from "asc-web-components";
import UserContent from "./userContent";
import { setUserContextOptions } from "../../../../../store/people/actions";

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
  setUserContextOptions,
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

  const setContextOptions = () => {
    setUserContextOptions(contextOptionsProps.contextOptions);
  };

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
      onContextMenu={setContextOptions}
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

const mapStateToProps = (state) => {
  const { filter } = state.people;

  return {
    filter,
  };
};

export default connect(mapStateToProps, {
  setUserContextOptions,
})(PeopleRow);
