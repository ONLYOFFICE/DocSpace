import React from "react";

// @ts-ignore
import { getFolder, getFolderInfo } from "@docspace/common/api/files";
// @ts-ignore
import FilesFilter from "@docspace/common/api/files/filter";
// @ts-ignore
import { iconSize32 } from "@docspace/common/utils/image-helpers";

import { PAGE_COUNT, defaultBreadCrumb } from "../utils";

import {
  useFilesHelpersProps,
  Item,
  BreadCrumb,
  Security,
} from "../FilesSelector.types";
import {
  ApplyFilterOption,
  FilesSelectorFilterTypes,
  FilterType,
  FolderType,
} from "@docspace/common/constants";

const getIconUrl = (extension: string, isImage: boolean, isMedia: boolean) => {
  // if (extension !== iconPath) return iconSize32.get(iconPath);
  let path = "";

  switch (extension) {
    case ".avi":
      path = "avi.svg";
      break;
    case ".csv":
      path = "csv.svg";
      break;
    case ".djvu":
      path = "djvu.svg";
      break;
    case ".doc":
      path = "doc.svg";
      break;
    case ".docm":
      path = "docm.svg";
      break;
    case ".docx":
      path = "docx.svg";
      break;
    case ".dotx":
      path = "dotx.svg";
      break;
    case ".dvd":
      path = "dvd.svg";
      break;
    case ".epub":
      path = "epub.svg";
      break;
    case ".pb2":
    case ".fb2":
      path = "fb2.svg";
      break;
    case ".flv":
      path = "flv.svg";
      break;
    case ".fodt":
      path = "fodt.svg";
      break;
    case ".iaf":
      path = "iaf.svg";
      break;
    case ".ics":
      path = "ics.svg";
      break;
    case ".m2ts":
      path = "m2ts.svg";
      break;
    case ".mht":
      path = "mht.svg";
      break;
    case ".mkv":
      path = "mkv.svg";
      break;
    case ".mov":
      path = "mov.svg";
      break;
    case ".mp4":
      path = "mp4.svg";
      break;
    case ".mpg":
      path = "mpg.svg";
      break;
    case ".odp":
      path = "odp.svg";
      break;
    case ".ods":
      path = "ods.svg";
      break;
    case ".odt":
      path = "odt.svg";
      break;
    case ".otp":
      path = "otp.svg";
      break;
    case ".ots":
      path = "ots.svg";
      break;
    case ".ott":
      path = "ott.svg";
      break;
    case ".pdf":
      path = "pdf.svg";
      break;
    case ".pot":
      path = "pot.svg";
      break;
    case ".pps":
      path = "pps.svg";
      break;
    case ".ppsx":
      path = "ppsx.svg";
      break;
    case ".ppt":
      path = "ppt.svg";
      break;
    case ".pptm":
      path = "pptm.svg";
      break;
    case ".pptx":
      path = "pptx.svg";
      break;
    case ".rtf":
      path = "rtf.svg";
      break;
    case ".svg":
      path = "svg.svg";
      break;
    case ".txt":
      path = "txt.svg";
      break;
    case ".webm":
      path = "webm.svg";
      break;
    case ".xls":
      path = "xls.svg";
      break;
    case ".xlsm":
      path = "xlsm.svg";
      break;
    case ".xlsx":
      path = "xlsx.svg";
      break;
    case ".xps":
      path = "xps.svg";
      break;
    case ".xml":
      path = "xml.svg";
      break;
    case ".oform":
      path = "oform.svg";
      break;
    case ".docxf":
      path = "docxf.svg";
      break;
    default:
      path = "file.svg";
      break;
  }

  if (isMedia) path = "sound.svg";
  if (isImage) path = "image.svg";

  return iconSize32.get(path);
};

const convertFoldersToItems = (
  folders: any,
  disabledItems: any[],
  filterParam?: string
) => {
  const items = folders.map((room: any) => {
    const {
      id,
      title,
      filesCount,
      foldersCount,
      security,
      parentId,
      rootFolderType,
    }: {
      id: number;
      title: string;
      filesCount: number;
      foldersCount: number;
      security: Security;
      parentId: number;
      rootFolderType: number;
    } = room;

    const icon = iconSize32.get("folder.svg");

    return {
      id,
      label: title,
      title,
      icon,
      filesCount,
      foldersCount,
      security,
      parentId,
      rootFolderType,
      isFolder: true,
      isDisabled: !!filterParam ? false : disabledItems.includes(id),
    };
  });

  return items;
};

const convertFilesToItems = (files: any, filterParam?: string) => {
  const items = files.map((file: any) => {
    const { id, title, security, parentId, rootFolderType, fileExst } = file;

    const isImage = file.viewAccessability.ImageView;
    const isMedia = file.viewAccessability.MediaView;

    let icon = getIconUrl(fileExst, isImage, isMedia);

    // if(filterParam)

    return {
      id,
      label: title.replace(fileExst, ""),
      title,
      icon,

      security,
      parentId,
      rootFolderType,
      isFolder: false,
      isDisabled: !filterParam,
    };
  });
  return items;
};

export const useFilesHelper = ({
  setIsNextPageLoading,
  setHasNextPage,
  setTotal,
  setItems,
  setBreadCrumbs,
  setIsBreadCrumbsLoading,
  isFirstLoad,
  selectedItemId,
  setIsRoot,
  searchValue,
  disabledItems,
  setSelectedItemSecurity,
  isThirdParty,
  onSelectTreeNode,
  setSelectedTreeNode,
  filterParam,
}: useFilesHelpersProps) => {
  const getFileList = React.useCallback(
    async (
      startIndex: number,
      itemId: number | string | undefined,
      isInit?: boolean,
      search?: string | null
    ) => {
      setIsNextPageLoading(true);

      const currentSearch = search
        ? search
        : search === null
        ? ""
        : searchValue || "";

      const page = startIndex / PAGE_COUNT;

      const filter = FilesFilter.getDefault();

      filter.page = page;
      filter.pageCount = PAGE_COUNT;
      filter.search = currentSearch;
      filter.applyFilterOption = null;
      filter.withSubfolders = false;
      if (filterParam) {
        filter.applyFilterOption = ApplyFilterOption.Files;
        switch (filterParam) {
          case FilesSelectorFilterTypes.DOCX:
            filter.filterType = FilterType.DocumentsOnly;
            break;

          case FilesSelectorFilterTypes.IMG:
            filter.filterType = FilterType.ImagesOnly;
            break;

          case FilesSelectorFilterTypes.GZ:
            filter.filterType = FilterType.ArchiveOnly;
            break;
        }
      }

      const id = itemId ? itemId : selectedItemId || "";

      filter.folder = id.toString();

      const currentFolder = await getFolder(id, filter);

      const { folders, files, total, count, pathParts, current } =
        currentFolder;

      setSelectedItemSecurity(current.security);

      const foldersList: Item[] = convertFoldersToItems(
        folders,
        disabledItems,
        filterParam
      );

      const filesList: Item[] = convertFilesToItems(files, filterParam);

      const itemList = [...foldersList, ...filesList];

      setHasNextPage(count === PAGE_COUNT);

      onSelectTreeNode && setSelectedTreeNode({ ...current, path: pathParts });

      if (isInit) {
        if (isThirdParty) {
          const breadCrumbs: BreadCrumb[] = [
            { label: current.title, isRoom: false, id: current.id },
          ];

          setBreadCrumbs(breadCrumbs);
          setIsBreadCrumbsLoading(false);
        } else {
          const breadCrumbs: BreadCrumb[] = await Promise.all(
            pathParts.map(async (folderId: number | string) => {
              const folderInfo: any = await getFolderInfo(folderId);

              const { title, id, parentId, rootFolderType } = folderInfo;

              return {
                label: title,
                id: id,
                isRoom: parentId === 0 && rootFolderType === FolderType.Rooms,
              };
            })
          );

          breadCrumbs.unshift({ ...defaultBreadCrumb });

          setBreadCrumbs(breadCrumbs);
          setIsBreadCrumbsLoading(false);
        }
      }

      if (isFirstLoad || startIndex === 0) {
        setTotal(total);
        setItems(itemList);
      } else {
        setItems((prevState: Item[] | null) => {
          if (prevState) return [...prevState, ...itemList];
          return [...itemList];
        });
      }
      setIsRoot(false);
      setIsNextPageLoading(false);
    },
    [selectedItemId, searchValue, isFirstLoad, disabledItems]
  );

  return { getFileList };
};

export default useFilesHelper;
