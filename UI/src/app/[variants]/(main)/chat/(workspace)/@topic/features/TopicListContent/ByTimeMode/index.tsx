'use client';

import isEqual from 'fast-deep-equal';
import React, { memo, useCallback, useMemo, useRef } from 'react';
import { useTranslation } from 'react-i18next';
import { GroupedVirtuoso, VirtuosoHandle } from 'react-virtuoso';

import { useChatStore } from '@/store/chat';
import { topicSelectors } from '@/store/chat/selectors';

import TopicItem from '../TopicItem';
import TopicGroupItem from './GroupItem';

const ByTimeMode = memo(() => {
  const { t } = useTranslation('topic');
  const virtuosoRef = useRef<VirtuosoHandle>(null);
  const [activeTopicId, activeThreadId] = useChatStore((s) => [s.activeTopicId, s.activeThreadId]);
  const groupTopics = useChatStore(topicSelectors.groupedTopicsSelector, isEqual);

  const { groups, groupCounts, topics } = useMemo(() => {
    return {
      groupCounts: groupTopics.map((group) => group.children.length),
      groups: groupTopics.map((group) => ({ id: group.id, title: group.title })),
      topics: groupTopics.flatMap((group) => group.children),
    };
  }, [groupTopics]);

  const itemContent = useCallback(
    (index: number) => {
      const { id, favorite, title } = topics[index];
      const uniqueKey = `${id}-${index}`;

      return (
        <TopicItem
          active={activeTopicId === id} // Now uses the same logic for all items
          fav={favorite}
          id={id}
          index={index}
          key={uniqueKey}
          threadId={activeThreadId}
          title={title}
        />
      );
    },
    [activeTopicId, topics, activeThreadId],
  );

  const groupContent = useCallback(
    (index: number) => {
      const topicGroup = groups[index];
      return <TopicGroupItem {...topicGroup} />;
    },
    [groups],
  );

  return (
    <GroupedVirtuoso
      groupContent={groupContent}
      groupCounts={groupCounts}
      itemContent={itemContent}
      ref={virtuosoRef}
    />
  );
});

ByTimeMode.displayName = 'ByTimeMode';

export default ByTimeMode;