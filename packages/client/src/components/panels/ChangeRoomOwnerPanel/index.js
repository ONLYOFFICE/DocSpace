import React, { useState, useEffect } from "react";
import { inject, observer } from "mobx-react";
import Aside from "@docspace/components/aside";
import Backdrop from "@docspace/components/backdrop";
import PeopleSelector from "@docspace/client/src/components/PeopleSelector";
import { withTranslation } from "react-i18next";
import Filter from "@docspace/common/api/people/filter";
import { EmployeeType } from "@docspace/common/constants";
import toastr from "@docspace/components/toast/toastr";

const ChangeRoomOwner = (props) => {
  const { t, visible, setIsVisible, setLeaveRoomDialogVisible } = props;

  const [isLoading, setIsLoading] = useState(false);

  useEffect(() => {
    document.addEventListener("keyup", onKeyUp, false);

    return () => {
      document.removeEventListener("keyup", onKeyUp, false);
    };
  }, []);

  const onKeyUp = (e) => {
    if (e.keyCode === 27) onClose();
    if (e.keyCode === 13 || e.which === 13) onChangeRoomOwner();
  };

  const onChangeRoomOwner = () => {
    setIsLoading(true);
    console.log("onChangeRoomOwnerAction");
    onClose();
    setIsLoading(false);
  };

  const onClose = () => {
    setIsVisible(false);
  };

  const onBackClick = () => {
    onClose();
    setLeaveRoomDialogVisible(true);
  };

  const filter = new Filter();
  filter.role = EmployeeType.Admin; // 1(EmployeeType.User) - RoomAdmin | 3(EmployeeType.Admin) - DocSpaceAdmin

  return (
    <>
      <Backdrop
        onClick={onClose}
        visible={visible}
        zIndex={310}
        isAside={true}
      />
      <Aside
        className="header_aside-panel"
        visible={visible}
        onClose={onClose}
        withoutBodyScroll
      >
        <PeopleSelector
          withCancelButton
          onBackClick={onBackClick}
          onAccept={onChangeRoomOwner}
          onCancel={onClose}
          acceptButtonLabel={t("Files:AssignAnOwner")}
          headerLabel={t("Files:ChangeOfRoomOwner")}
          filter={filter}
          isLoading={isLoading}
        />
      </Aside>
    </>
  );
};

export default inject(({ dialogsStore }) => {
  const {
    changeRoomOwnerIsVisible,
    setChangeRoomOwnerIsVisible,
    setLeaveRoomDialogVisible,
  } = dialogsStore;

  return {
    visible: changeRoomOwnerIsVisible,
    setIsVisible: setChangeRoomOwnerIsVisible,
    setLeaveRoomDialogVisible,
  };
})(observer(withTranslation(["Files"])(ChangeRoomOwner)));
