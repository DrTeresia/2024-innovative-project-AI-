import math
import sys
from operator import itemgetter

sys.path.append('../../')


def perceive(persona, map):
    """
    :param persona: 将领 
    :param map: 地图
    :return: 决策
    将领获取周围环境信息（地形，敌人）
    """
    nearby_tiles = map.get_nearby_tiles(persona.scratch.curr_tile,
                                       persona.vision_r)

    for i in nearby_tiles:
        i = map.access_tile(i)
        if i["world"]:
            if (i["world"] not in persona.s_mem.tree):
                persona.s_mem.tree[i["world"]] = {}
        if i["sector"]:
            if (i["sector"] not in persona.s_mem.tree[i["world"]]):
                persona.s_mem.tree[i["world"]][i["sector"]] = {}
        if i["arena"]:
            if (i["arena"] not in persona.s_mem.tree[i["world"]]
            [i["sector"]]):
                persona.s_mem.tree[i["world"]][i["sector"]][i["arena"]] = []
        if i["game_object"]:
            if (i["game_object"] not in persona.s_mem.tree[i["world"]]
            [i["sector"]]
            [i["arena"]]):
                persona.s_mem.tree[i["world"]][i["sector"]][i["arena"]] += [
                    i["game_object"]]


    curr_block_path = map.get_tile_path(persona.scratch.curr_tile, "block")
    # We do not perceive the same event twice (this can happen if an object is
    # extended across multiple tiles).
    percept_events_set = set()
    # We will order our percept based on the distance, with the closest ones
    # getting priorities.
    percept_events_list = []
    # First, we put all events that are occuring in the nearby tiles into the
    # percept_events_list

    for info in nearby_tiles:
        info_details = map.access_info(info)
        if info_details["events"]:
            if map.get_tile_path(info, "block") == curr_block_path:
                # This calculates the distance between the persona's current tile,
                # and the target tile.
                dist = math.dist([info[0], info[1]],
                                 [persona.scratch.curr_tile[0],
                                  persona.scratch.curr_tile[1]])
                # Add any relevant events to our temp set/list with the distant info.
                for event in info_details["events"]:
                    if event not in percept_events_set:
                        percept_events_list += [[dist, event]]
                        percept_events_set.add(event)

    # # We sort, and perceive only persona.scratch.att_bandwidth of the closest
    # # events. If the bandwidth is larger, then it means the persona can perceive
    # # more elements within a small area.
    # percept_events_list = sorted(percept_events_list, key=itemgetter(0))
    # perceived_events = []
    # for dist, event in percept_events_list[:persona.scratch.att_bandwidth]:
    #     perceived_events += [event]
    #
    # # Storing events.
    # # <ret_events> is a list of <ConceptNode> instances from the persona's
    # # associative memory.
    # ret_events = []
    # for p_event in perceived_events:
    #     s, p, o, desc = p_event
    #     if not p:
    #         # If the object is not present, then we default the event to "idle".
    #         p = "is"
    #         o = "idle"
    #         desc = "idle"
    #     desc = f"{s.split(':')[-1]} is {desc}"
    #     p_event = (s, p, o)
    #
    #     # We retrieve the latest persona.scratch.retention events. If there is
    #     # something new that is happening (that is, p_event not in latest_events),
    #     # then we add that event to the a_mem and return it.
    #     latest_events = persona.a_mem.get_summarized_latest_events(
    #         persona.scratch.retention)
    #     if p_event not in latest_events:
    #         # We start by managing keywords.
    #         keywords = set()
    #         sub = p_event[0]
    #         obj = p_event[2]
    #         if ":" in p_event[0]:
    #             sub = p_event[0].split(":")[-1]
    #         if ":" in p_event[2]:
    #             obj = p_event[2].split(":")[-1]
    #         keywords.update([sub, obj])
    #
    #         # Get event embedding
    #         desc_embedding_in = desc
    #         if "(" in desc:
    #             desc_embedding_in = (desc_embedding_in.split("(")[1]
    #                                  .split(")")[0]
    #                                  .strip())
    #         if desc_embedding_in in persona.a_mem.embeddings:
    #             event_embedding = persona.a_mem.embeddings[desc_embedding_in]
    #         else:
    #             event_embedding = get_embedding(desc_embedding_in)
    #         event_embedding_pair = (desc_embedding_in, event_embedding)
    #
    #         # Get event poignancy.
    #         event_poignancy = generate_poig_score(persona,
    #                                               "event",
    #                                               desc_embedding_in)
    #
    #         # If we observe the persona's self chat, we include that in the memory
    #         # of the persona here.
    #         chat_node_ids = []
    #         if p_event[0] == f"{persona.name}" and p_event[1] == "chat with":
    #             curr_event = persona.scratch.act_event
    #             if persona.scratch.act_description in persona.a_mem.embeddings:
    #                 chat_embedding = persona.a_mem.embeddings[
    #                     persona.scratch.act_description]
    #             else:
    #                 chat_embedding = get_embedding(persona.scratch
    #                                                .act_description)
    #             chat_embedding_pair = (persona.scratch.act_description,
    #                                    chat_embedding)
    #             chat_poignancy = generate_poig_score(persona, "chat",
    #                                                  persona.scratch.act_description)
    #             chat_node = persona.a_mem.add_chat(persona.scratch.curr_time, None,
    #                                                curr_event[0], curr_event[1], curr_event[2],
    #                                                persona.scratch.act_description, keywords,
    #                                                chat_poignancy, chat_embedding_pair,
    #                                                persona.scratch.chat)
    #             chat_node_ids = [chat_node.node_id]
    #
    #         # Finally, we add the current event to the agent's memory.
    #         ret_events += [persona.a_mem.add_event(persona.scratch.curr_time, None,
    #                                                s, p, o, desc, keywords, event_poignancy,
    #                                                event_embedding_pair, chat_node_ids)]
    #         persona.scratch.importance_trigger_curr -= event_poignancy
    #         persona.scratch.importance_ele_n += 1

    # 返回值是周围环境信息
    # return ret_events


