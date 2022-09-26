import React, { useState } from "react";

import Text from "@docspace/components/text";
import Link from "@docspace/components/link";
import IconButton from "@docspace/components/icon-button";
import { ReactSVG } from "react-svg";
import {
  StyledHistoryBlockFile,
  StyledHistoryBlockFilesList,
  StyledUserNameLink,
} from "../../styles/history";
import { RoomsType } from "@docspace/common/constants";

export const HistoryBlockItemList = ({
  t,
  items,
  getItemIcon,
  openFileAction,
}) => {
  const [isShowMore, setIsShowMore] = useState(items.length <= 3);
  const onShowMore = () => setIsShowMore(true);

  const changeUrl = (item) => {
    const id = item.ExtraLocationUrl.split("folderid=")[1];
    const viewUrl = item.ExtraLocationUrl;

    if (!id || !viewUrl) return;
    openFileAction({ id, viewUrl, fileExst: item.fileExst, isFolder: true });
  };

  const parsedItems = items.map((item) => {
    const splitTitle = item.Title.split(".");
    return {
      ...item,
      isRoom: item.Item === "room",
      isFolder: item.Item === "folder",
      roomType: RoomsType[item.AdditionalInfo],
      title: splitTitle[0],
      fileExst: splitTitle[1] ? `.${splitTitle[1]}` : null,
      id: item.ItemId.split("_")[0],
      viewUrl: item.itemId,
    };
  });

  return (
    <StyledHistoryBlockFilesList>
      {parsedItems.map((item, i) => {
        if (!isShowMore && i > 2) return null;
        return (
          <StyledHistoryBlockFile isRoom={item.isRoom} key={i}>
            <ReactSVG className="icon" src={getItemIcon(item, 24)} />
            <div className="item-title">
              <span className="name">{item.title}</span>
              {item.fileExst && <span className="exst">{item.fileExst}</span>}
            </div>
            <IconButton
              className="location-btn"
              iconName="/static/images/folder-location.react.svg"
              size="16"
              isFill={true}
              onClick={() => changeUrl(item)}
            />
          </StyledHistoryBlockFile>
        );
      })}
      {!isShowMore && (
        <Text className="show_more-link" onClick={onShowMore}>
          {t("Common:ShowMore")}
        </Text>
      )}
    </StyledHistoryBlockFilesList>
  );
};

// const HistoryBlockContent = ({ t, feed }) => {
//   return (
//     <StyledHistoryBlockContent>
//       {action === "message" ? (
//         <Text>{details.message}</Text>
//       ) : action === "appointing" ? (
//         <div className="appointing">
//           {`${t("appointed")}
//             ${details.appointedRole} `}{" "}
//           <UserNameLink user={details.appointedUser} />
//         </div>
//       ) : action === "users" ? (
//         <div className="users">
//           <div className="user-list">
//             added users{" "}
//             {details.users.map((user, i) => (
//               <UserNameLink
//                 key={user.id}
//                 user={user}
//                 withComma={i + 1 !== details.users.length}
//               />
//             ))}
//           </div>
//         </div>
//       ) : action === "files" ? (
//         <div className="files">
//           <Text>add new 2 files into the folder “My project”</Text>
//           <div className="files-list">
//             {details.files.map((file) => (
//               <div className="file" key={file.id}>
//                 <ReactSVG className="icon" src={file.icon} />
//                 <div className="file-title">
//                   <span className="name">{file.title.split(".")[0]}</span>
//                   <span className="exst">{file.fileExst}</span>
//                 </div>
//                 <IconButton
//                   className="location-btn"
//                   iconName="/static/images/folder-location.react.svg"
//                   size="16"
//                   isFill={true}
//                   onClick={() => {}}
//                 />
//               </div>
//             ))}
//           </div>
//         </div>
//       ) : null}
//     </StyledHistoryBlockContent>
//   );
// };

// const UserNameLink = ({ user, withComma }) => {
//   const username = user.displayName || user.email;
//   const space = <div className="space"></div>;

//   return (
//     <StyledUserNameLink className="user">
//       {user.profileUrl ? (
//         <Link
//           className="username link"
//           isHovered
//           type="action"
//           href={user.profileURl}
//         >
//           {username}
//           {withComma ? "," : ""}
//           {withComma && space}
//         </Link>
//       ) : (
//         <div className="username text" key={user.id}>
//           {username}
//           {withComma ? "," : ""}
//           {withComma && space}
//         </div>
//       )}
//     </StyledUserNameLink>
//   );
// };

export default HistoryBlockItemList;
