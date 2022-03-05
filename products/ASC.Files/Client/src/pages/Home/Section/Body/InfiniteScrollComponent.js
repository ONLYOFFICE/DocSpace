import React from "react";
import InfiniteScroll from "react-infinite-scroll-component";
import { inject, observer } from "mobx-react";
import api from "@appserver/common/api";

const InfiniteScrollComponent = ({
  filesList,
  viewAs,
  setViewAs,
  setFirsElemChecked,
}) => {
  const fetchMoreData = () => {
    const newFilter = filter;
    newFilter.page = newFilter.page++;

    api.files.getFolder(filter.folder, newFilter);
    //items: this.state.items.concat(Array.from({ length: 20 }))
  };

  return (
    <InfiniteScroll
      //dataLength={this.state.items.length}
      dataLength={filesList.length}
      next={fetchMoreData}
      //hasMore={this.state.hasMore}
      hasMore={false}
      loader={<h4>Loading...</h4>}
      endMessage={
        <p style={{ textAlign: "center" }}>
          <b>Yay! You have seen it all</b>
        </p>
      }
    >
      {filesList.map((i, index) => (
        <div
          style={{
            height: 30,
            border: "1px solid green",
            margin: 6,
            padding: 8,
          }}
          key={index}
        >
          div - #{index}
        </div>
      ))}
    </InfiniteScroll>
  );
};

export default inject(({ filesStore, auth }) => {
  const {
    filesList,
    viewAs,
    setViewAs,
    setFirsElemChecked,
    filter,
  } = filesStore;

  console.log("filter", filter);

  return {
    filesList,
    viewAs,
    setViewAs,
    setFirsElemChecked,
    theme: auth.settingsStore.theme,
    filter,
  };
})(observer(InfiniteScrollComponent));
