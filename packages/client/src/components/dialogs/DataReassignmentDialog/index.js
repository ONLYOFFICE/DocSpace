import { useState, useEffect } from "react";
import styled from "styled-components";
import { inject, observer } from "mobx-react";
import { withTranslation } from "react-i18next";
import PeopleSelector from "@docspace/client/src/components/PeopleSelector";
import ModalDialog from "@docspace/components/modal-dialog";
import toastr from "@docspace/components/toast/toastr";
import { useNavigate } from "react-router-dom";
import Backdrop from "@docspace/components/backdrop";
import Body from "./sub-components/Body";
import Footer from "./sub-components/Footer";
import api from "@docspace/common/api";
const { Filter } = api;

const StyledModalDialog = styled(ModalDialog)`
  .avatar-name,
  .delete-profile-container {
    display: flex;
    align-items: center;
  }

  .delete-profile-checkbox {
    margin-bottom: 16px;
  }

  .list-container {
    gap: 6px;
  }
`;

const statusTerminateCompleted = 3;
let timerId;

const DataReassignmentDialog = ({
  visible,
  user,
  setDataReassignmentDialogVisible,
  dataReassignment,
  dataReassignmentProgress,
  dataReassignmentTerminate,
  currentColorScheme,
  currentUser,
  deleteProfile,
  isDeletingUserWithReassignment,
  t,
  tReady,
  getUsersList,
  setIsDeletingUserWithReassignment,
  setDataReassignmentDeleteProfile,
  dataReassignmentUrl,
}) => {
  const [selectorVisible, setSelectorVisible] = useState(false);
  const defaultSelectedUser = isDeletingUserWithReassignment
    ? currentUser
    : null;
  const [selectedUser, setSelectedUser] = useState(defaultSelectedUser);
  const [isDeleteProfile, setIsDeleteProfile] = useState(deleteProfile);
  const [showProgress, setShowProgress] = useState(false);
  const [isReassignCurrentUser, setIsReassignCurrentUser] = useState(false);
  const [isAbortTransfer, setIsAbortTransfer] = useState(false);

  const [percent, setPercent] = useState(0);

  const updateAccountsAfterDeleteUser = () => {
    const filter = Filter.getDefault();
    getUsersList(filter, true);
    return;
  };

  useEffect(() => {
    //If click Delete user
    if (isDeletingUserWithReassignment) onReassign();

    return () => {
      setIsDeletingUserWithReassignment(false);
      setDataReassignmentDeleteProfile(false);
      clearTimeout(timerId);
    };
  }, [isDeletingUserWithReassignment]);

  const onToggleDeleteProfile = () => {
    setIsDeleteProfile((remove) => !remove);
  };

  const onTogglePeopleSelector = () => {
    setSelectorVisible((show) => !show);
  };

  const onClose = () => {
    setDataReassignmentDialogVisible(false);
  };

  const onClosePeopleSelector = () => {
    setSelectorVisible(false);
  };

  const onStartAgain = () => {
    setShowProgress(false);
    setPercent(0);
    setIsAbortTransfer(false);
  };

  const onAccept = (item) => {
    setSelectorVisible(false);
    setSelectedUser({ ...item[0] });
  };

  const checkReassignCurrentUser = () => {
    setIsReassignCurrentUser(currentUser.id === selectedUser.id);
  };

  const checkProgress = () => {
    dataReassignmentProgress(user.id)
      .then((res) => {
        //If the task has already been interrupted and killed
        if (!res) return;

        if (res.error) {
          toastr.error(res.error);
          return;
        }

        setPercent(res.percentage);

        if (!res.isCompleted) {
          timerId = setTimeout(checkProgress, 500);
          return;
        }

        clearTimeout(timerId);
        if (res.status === statusTerminateCompleted) return;

        toastr.success(t("Common:ChangesSavedSuccessfully"));
        isDeleteProfile && updateAccountsAfterDeleteUser();
      })
      .catch((error) => {
        toastr.error(error?.response?.data?.error?.message);
      });
  };

  const onReassign = () => {
    checkReassignCurrentUser();
    setShowProgress(true);

    dataReassignment(user.id, selectedUser.id, isDeleteProfile)
      .then(() => checkProgress())
      .catch((error) => {
        toastr.error(error?.response?.data?.error?.message);
      });
  };

  const onTerminate = () => {
    clearTimeout(timerId);

    dataReassignmentTerminate(user.id)
      .then((res) => {
        setPercent(res.percentage);
        setIsAbortTransfer(true);
        toastr.success(t("Common:ChangesSavedSuccessfully"));
        isDeleteProfile && updateAccountsAfterDeleteUser();
      })
      .catch((error) => {
        toastr.error(error?.response?.data?.error?.message);
      });
  };

  if (selectorVisible) {
    return (
      <StyledModalDialog
        displayType="aside"
        visible={visible}
        onClose={onClosePeopleSelector}
        containerVisible={selectorVisible}
        withFooterBorder={true}
        withBodyScroll={true}
      >
        <Backdrop
          onClick={onClosePeopleSelector}
          visible={selectorVisible}
          isAside={true}
        />
        <ModalDialog.Container>
          <PeopleSelector
            acceptButtonLabel={t("Common:SelectAction")}
            excludeItems={[user.id]}
            onAccept={onAccept}
            onCancel={onClosePeopleSelector}
            onBackClick={onTogglePeopleSelector}
            withCancelButton={true}
            withAbilityCreateRoomUsers={true}
          />
        </ModalDialog.Container>
      </StyledModalDialog>
    );
  }

  return (
    <StyledModalDialog
      displayType="aside"
      visible={visible}
      onClose={onClose}
      containerVisible={selectorVisible}
      withFooterBorder={true}
      withBodyScroll={true}
    >
      <ModalDialog.Header>
        {t("DataReassignmentDialog:DataReassignment")}
      </ModalDialog.Header>
      <ModalDialog.Body>
        <Body
          t={t}
          tReady={tReady}
          showProgress={showProgress}
          isReassignCurrentUser={isReassignCurrentUser}
          user={user}
          selectedUser={selectedUser}
          percent={percent}
          isAbortTransfer={isAbortTransfer}
          dataReassignmentUrl={dataReassignmentUrl}
          currentColorScheme={currentColorScheme}
          onTogglePeopleSelector={onTogglePeopleSelector}
        />
      </ModalDialog.Body>

      <ModalDialog.Footer>
        <Footer
          t={t}
          showProgress={showProgress}
          isDeleteProfile={isDeleteProfile}
          onToggleDeleteProfile={onToggleDeleteProfile}
          selectedUser={selectedUser}
          onReassign={onReassign}
          percent={percent}
          isAbortTransfer={isAbortTransfer}
          onClose={onClose}
          onTerminate={onTerminate}
          onStartAgain={onStartAgain}
        />
      </ModalDialog.Footer>
    </StyledModalDialog>
  );
};

export default inject(({ auth, peopleStore, setup }) => {
  const {
    setDataReassignmentDialogVisible,
    dataReassignmentDeleteProfile,
    setDataReassignmentDeleteProfile,
    isDeletingUserWithReassignment,
    setIsDeletingUserWithReassignment,
  } = peopleStore.dialogStore;
  const { currentColorScheme, dataReassignmentUrl } = auth.settingsStore;
  const {
    dataReassignment,
    dataReassignmentProgress,
    dataReassignmentTerminate,
  } = setup;

  const { user: currentUser } = peopleStore.authStore.userStore;

  const { getUsersList } = peopleStore.usersStore;

  return {
    setDataReassignmentDialogVisible,
    theme: auth.settingsStore.theme,
    currentColorScheme,
    dataReassignment,
    currentUser,
    dataReassignmentProgress,
    dataReassignmentTerminate,
    deleteProfile: dataReassignmentDeleteProfile,
    setDataReassignmentDeleteProfile,
    getUsersList,
    isDeletingUserWithReassignment,
    setIsDeletingUserWithReassignment,
    dataReassignmentUrl,
  };
})(
  observer(
    withTranslation([
      "Common",
      "DataReassignmentDialog",
      "Translations",
      "ChangePortalOwner",
    ])(DataReassignmentDialog)
  )
);
