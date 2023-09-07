import Progress from "./Progress";
import Loader from "./Loader";
import AccountInfo from "./AccountInfo";
import Description from "./Description";
import NewOwner from "./NewOwner";

const Body = ({
  t,
  tReady,
  showProgress,
  isReassignCurrentUser,
  user,
  selectedUser,
  percent,
  currentColorScheme,
  onTogglePeopleSelector,
}) => {
  if (!tReady) return <Loader />;

  if (showProgress)
    return (
      <Progress
        isReassignCurrentUser={isReassignCurrentUser}
        fromUser={user.displayName}
        toUser={selectedUser.label}
        percent={percent}
      />
    );

  return (
    <>
      <AccountInfo user={user} />
      <NewOwner
        t={t}
        selectedUser={selectedUser}
        currentColorScheme={currentColorScheme}
        onTogglePeopleSelector={onTogglePeopleSelector}
      />
      <Description t={t} />
    </>
  );
};

export default Body;
