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

let timerId = null;

const DataReassignmentDialog = ({
  visible,
  user,
  setDataReassignmentDialogVisible,
  dataReassignment,
  dataReassignmentProgress,
  currentColorScheme,
  currentUser,
  deleteProfile,
  isDeleteUserReassignmentYourself,
  t,
  tReady,
  setFilter,
  setIsDeleteUserReassignmentYourself,
}) => {
  const [selectorVisible, setSelectorVisible] = useState(false);
  const defaultSelectedUser = isDeleteUserReassignmentYourself
    ? currentUser
    : null;
  const [selectedUser, setSelectedUser] = useState(defaultSelectedUser);
  const [isDeleteProfile, setIsDeleteProfile] = useState(deleteProfile);
  const [showProgress, setShowProgress] = useState(false);
  const [isReassignCurrentUser, setIsReassignCurrentUser] = useState(false);

  const [percent, setPercent] = useState(0);

  const navigate = useNavigate();

  const updateAccountsAfterDeleteUser = () => {
    const filter = Filter.getDefault();
    const params = filter.toUrlParams();
    const url = `/accounts/filter?${params}`;

    navigate(url, params);
    setFilter(filter);
    return;
  };

  useEffect(() => {
    //If click Delete user
    if (isDeleteUserReassignmentYourself) onReassign();

    return () => {
      setIsDeleteUserReassignmentYourself(false);
      clearInterval(timerId);
    };
  }, [isDeleteUserReassignmentYourself]);

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

  const onAccept = (item) => {
    setSelectorVisible(false);
    setSelectedUser({ ...item[0] });
  };

  const checkReassignCurrentUser = () => {
    setIsReassignCurrentUser(currentUser.id === selectedUser.id);
  };

  const checkProgress = (id) => {
    dataReassignmentProgress(id)
      .then((res) => {
        setPercent(res.percentage);

        if (res.percentage !== 100) return;

        clearInterval(timerId);
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
      .then(() => {
        toastr.success(t("Common:ChangesSavedSuccessfully"));

        timerId = setInterval(() => checkProgress(user.id), 1000);
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
        withFooterBorder
        withBodyScroll
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
      withFooterBorder
      withBodyScroll
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
          onClose={onClose}
        />
      </ModalDialog.Footer>
    </StyledModalDialog>
  );
};

export default inject(({ auth, peopleStore, setup }) => {
  const {
    setDataReassignmentDialogVisible,
    dataReassignmentDeleteProfile,
    isDeleteUserReassignmentYourself,
    setIsDeleteUserReassignmentYourself,
  } = peopleStore.dialogStore;
  const { currentColorScheme } = auth.settingsStore;
  const { dataReassignment, dataReassignmentProgress } = setup;

  const { user: currentUser } = peopleStore.authStore.userStore;

  const { setFilterParams: setFilter } = peopleStore.filterStore;

  return {
    setDataReassignmentDialogVisible,
    theme: auth.settingsStore.theme,
    currentColorScheme,
    dataReassignment,
    currentUser,
    dataReassignmentProgress,
    deleteProfile: dataReassignmentDeleteProfile,
    setFilter,
    isDeleteUserReassignmentYourself,
    setIsDeleteUserReassignmentYourself,
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
